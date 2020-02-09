
const express = require('express')
import {Request, Response} from 'express'
const router = express.Router()

const uuid = require('uuid/v4')
import {createJwt} from '../token/generateToken'

router.use(function requestLog (req:Request, res:Response, next:any) {
    console.log({sessionID: uuid(), method: req.method, url: req.url, host: req.hostname})
    next()
  });

router.get('/api', (req:Request, res:Response) => {

    res.json('Hello from API')
});

router.get('/api/token', async (req:Request, res:Response) => {
    
    res.status(200).json(await createJwt());
   
});

module.exports = router;



