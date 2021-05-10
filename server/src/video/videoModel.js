const mongoose = require('mongoose');

var videoSchema = mongoose.Schema({
    name: {
        type: String,
        require: true
    },
    create_date: {
        type: Date,
        default: Date.now
    }
})

const Video = module.exports = mongoose.model('video', videoSchema);
module.exports.get = (callback, limit) => Video.find(callback).limit(limit);