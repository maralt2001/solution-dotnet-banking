"use strict";
const express = require('express');
const Port = 5010;
const app = express();
const router = require('./router/apiRouter');
app.use(router);
app.listen(Port, () => console.log('Server started...'));
//# sourceMappingURL=app.js.map