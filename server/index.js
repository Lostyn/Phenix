const express = require('express');
const apiKeyAuth = require('api-key-auth');
const mongoose = require('mongoose');
const app = express()

const apiRoutes = require("./src/api-routes")
app.use(express.urlencoded({ extended: true}));
app.use(express.json());

mongoose.connect('mongodb://localhost/phenix', { useNewUrlParser: true, useUnifiedTopology: true });
const db = mongoose.connection;
db.on('error', (err) => { console.log("error") });
db.once('open', () => {
    console.log("Db connected successfully")

    var port = process.env.PORT || 8080;
    app.get('/', (req, res) => res.send('Hello World with Express !'));
    
    // Auth
    app.use(apiKeyAuth({getSecret}));

    // Parse req
    app.use( express.urlencoded({ extended: true }) );
    app.use( express.json() );
    app.use('/api', apiRoutes);

    app.listen(port, () => {
        console.log("Running on port " + port);
    });
})

const apiKeys = new Map();
apiKeys.set('123456789', { id: 1, name: 'vincent', secret: 'secret1'});
function getSecret(keyId, cb) {
    if (!apiKeys.has(keyId))
        return cb(new Error('Unknown api key'));

    const clientApp = apiKeys.get(keyId);
    cb(null, clientApp.secret, {
        id: clientApp.id,
        name: clientApp.name
    });
}