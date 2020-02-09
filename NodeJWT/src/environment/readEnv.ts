
const fs = require('fs')
export async function readApiEnvironmentSettings():Promise<any> {
    return new Promise((resolve, reject) => {
      fs.readFile('../BankingApi/appsettings.json', 'utf8', function (err:any, data:any) {
        if (err) {
          reject(err);
        }
        resolve(JSON.parse(data));
      });
    });
  }

  