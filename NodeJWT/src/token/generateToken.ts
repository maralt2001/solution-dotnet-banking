
const jwt = require('jsonwebtoken')

export function createJwt(environment:any):JsonWebKey {
   
    return jwt.sign({audience: environment.audience, issuer: environment.issuer},environment.securityKey, { expiresIn: 60 * 60 })    
    
}


