const mongoose = require('mongoose');

var appSchema = mongoose.Schema({
    name: { type: String, require: true },
    appId: { type: String, require: true },
    secret: { type: String, require: true },
    create_date: { type: Date, default: Date.now },
    settings: {
        logo: { type: String },
        panorama: { type: String },
        ui_mode: { type: String, default: "frame" },
        vr_mode: { type: String, default: "gaze" }
    }
});

const App = module.exports = mongoose.model('app', appSchema);
module.exports.get = (callback, limit) => App.find(callback).limit(limit);