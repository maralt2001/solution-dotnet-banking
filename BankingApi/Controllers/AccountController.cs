using System;
using System.Threading.Tasks;
using BankingApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoService;
using ApiAccess;


namespace BankingApi.Controllers
{

    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IDBContext _dBContext;

        // constructor using DI Injection IDBContext und ILogger
        public AccountController(IDBContext dbContext, ILogger<AccountController> logger)
        {
            _logger = logger;
            _dBContext = dbContext;

        }

        #region Url Path /api/check/dbconnection

        [HttpGet]
        [Route("api/check/dbconnection")]
        [Produces("application/json")]

        public async Task<IActionResult> CheckDBConnection()
        {
            var result = await _dBContext.IsConnectionUp();
            return result ? Ok("DB Connection is up!") : Ok("DB Connection is down!");
        }

        #endregion

        #region Url Path /api/banking/account

        [HttpGet]
        [Route("api/banking/account/createFakeDocument")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateFakeBankingAccount()
        {
            var createAccount = Task.Run(() =>
                {
                    BankingAccount bankingAccount = new BankingAccount
                    {
                        firstname = "hans",
                        lastname = "schleicher",
                        isActive = true,
                        createdAt = DateTime.Now,
                        changed = new AccountChanged { changedBy = "markus", changedAt = DateTime.Now }
                    };
                    return bankingAccount;
                }
            );

            
            bool response = await _dBContext.InsertRecordAsync<BankingAccount>("Banking_Accounts", await createAccount);


            if (response)
            {
                return Ok(response);
            }
            else 
            {
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("api/banking/account/update")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateBankingAccount([FromQuery]string id,[FromBody] BankingAccount bankingAccount)
        {
            bankingAccount.AddChanged(DateTime.Now, "administrator");
            
            try
            {
                bool result = await _dBContext.UpdateRecordAsync<BankingAccount>("Banking_Accounts", id, bankingAccount);
                return Ok($"BankingAccount id: {id} update succeeded");
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

       
        [HttpPost]
        [Route("api/banking/account/create")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateBankingAccount([FromBody] BankingAccount bankingAccount)
        {
            bankingAccount.AddCreateAt(DateTime.Now);

            try
            {
                
                var result = await _dBContext.InsertRecordAsync<BankingAccount>("Banking_Accounts", bankingAccount);
                if (result)
                {
                    return Ok($"Account is created");
                }
                else
                {
                    return StatusCode(500);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete]
        [Route("api/banking/account/delete")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteBankingAccount([FromQuery] string id)
        {
            try
            {
                bool result = await _dBContext.DeleteRecordAsync<BankingAccount>("Banking_Accounts",id);
                if (result)
                {
                    return Ok($"{result} Banking Account is deleted");
                }
                else
                {
                    return StatusCode(500, "Delete of Banking Account failed");
                }
                

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/banking/account/checkFields")]
        [Produces("application/json")]
        public ActionResult CheckFields([FromBody] BankingAccount bankingAccount)
        {

            //deconstruct bankingAccount Object from Request
            var (id, firstname, lastname, isActive, createdAt, changed) = bankingAccount;

            return Ok($"{firstname}, {lastname}");
        }

        [HttpGet]
        [Route("api/banking/account/getone")]
        [Produces("application/json")]
        public async Task<IActionResult> GetOneAccount([FromQuery] string id)
        {
            try
            {
                BankingAccount result = await _dBContext.LoadRecordAsync<BankingAccount>("Banking_Accounts",id);
                return Ok(result);
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("api/banking/account/getoneregex")]
        [Produces("application/json")]
        public async Task<IActionResult> GetOneAccountRegex([FromQuery] string field, string regexvalue)
        {
            try
            {
                BankingAccount result = await _dBContext.LoadOneRecordRegexAsync<BankingAccount>("Banking_Accounts", field, regexvalue);
                return Ok(result);
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }
        
        #endregion

        #region Url Path /api/banking/accounts
        
        [HttpGet]
        [Route("api/banking/accounts/getall")]
        [Produces("application/json")]
        public async Task<IActionResult> GetBankingAccounts()
        {
            try
            {
                var result = await _dBContext.LoadRecordsAsync<BankingAccount>("Banking_Accounts");

                return Ok(result);
                
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("api/banking/accounts/take")]
        [Produces("application/json")]

        public async Task<IActionResult> GetBankingAccountsLimit([FromQuery] int limit, int skip = 0)
        {
            try
            {
               
                var result = await _dBContext.LoadRecordsSkpLimitAsync<BankingAccount>("Banking_Accounts",limit,skip);
                return Ok(result);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }

        #endregion



    }
}