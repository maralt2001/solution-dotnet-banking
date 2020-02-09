
const jwt = require('jsonwebtoken')


export function validateToken(token:JsonWebKey, env:any) {
    let decoded = jwt.verify(token, env.securityKey)
    return decoded;
}