App = require('./appModel');
const { uuidv4 } = require('../crypto');

const newId = () => Math.random().toString(36).substr(2, 12);
const newSecret = () => Math.random().toString(36).substr(2, 2);

exports.new = (req, res) => {
    const app = new App();
    app.name = req.body.name ? req.body.name : app.name;
    app.appId = newId();
    app.secret = uuidv4();

    app.save( err => {
        if (err) res.json(err);

        res.json({message: "New app created!", data: app});
    })
}

exports.delete = (req, res) => {
    App.remove({appId: req.params.appId}, (err, app) => {
        if (err) res.json(err);

        res.json({ status: 'success', message: 'App deleted'});
    });
}

exports.getManifest = (req, res) => {
    App.find({appId: req.params.appId}, (err, app) => {
        if (err) res.json(err);

        res.json({ statis: 'success', data: app});
    })
}