
const express = require('express')
import {Request, Response} from 'express'
const router = express.Router()

const uuid = require('uuid/v4')
import {createJwt} from '../token/generateToken'
import {validateToken} from '../token/validateToken'
import {readApiEnvironmentSettings} from '../environment/readEnv';

let env = readApiEnvironmentSettings().then(data => {return data});
    
router.use(express.json())
router.use(function requestLog (req:Request, res:Response, next:any) {
   console.log({sessionID: uuid(), method: req.method, url: req.url, host: req.hostname})
    next()
});

router.get('/api', (req:Request, res:Response) => {

    res.json('Hello from API')
});

router.get('/api/token', async (req:Request, res:Response) => {
    
    res.status(200).json(createJwt(await env));
   
});

router.post('/api/verifytoken', async (req:any, res:Response) => {
    let body:PostToken = req.body;
    var result = validateToken(body.token as JsonWebKey, await env)
    res.json(result)

});

class PostToken {
    token: string = '';
}

module.exports = router;



