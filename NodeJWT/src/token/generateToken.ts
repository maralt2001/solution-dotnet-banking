import { json } from "express";

const jwt = require('jsonwebtoken')
const fs = require('fs')

async function readApiEnvironmentSettings():Promise<any> {
    return new Promise((resolve, reject) => {
      fs.readFile('../BankingApi/appsettings.json', 'utf8', function (err:any, data:any) {
        if (err) {
          reject(err);
        }
        resolve(JSON.parse(data));
      });
    });
  }

export async function createJwt():Promise<JsonWebKey> {
    return readApiEnvironmentSettings().then(data => {
        let token:JsonWebKey = jwt.sign({issuer: data.issuer, audience: data.audience},data.securityKey, { expiresIn: 60 * 60 })    
        return token;
    })
}


