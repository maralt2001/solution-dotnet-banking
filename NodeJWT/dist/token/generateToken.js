"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const jwt = require('express-jwt');
function createJwt() {
    let token = jwt({ secret: 'hallo' });
    return token;
}
exports.createJwt = createJwt;
//# sourceMappingURL=generateToken.js.map