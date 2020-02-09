"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const express = require('express');
const router = express.Router();
const uuid = require('uuid/v4');
const generateToken_1 = require("../token/generateToken");
router.use(function requestLog(req, res, next) {
    console.log({ sessionID: uuid(), method: req.method, url: req.url });
    next();
});
router.get('/api', (req, res) => {
    res.json('Hello from API');
});
router.get('/api/token', (req, res) => {
    const result = generateToken_1.createJwt;
    res.send(result);
});
module.exports = router;
//# sourceMappingURL=apiRouter.js.map