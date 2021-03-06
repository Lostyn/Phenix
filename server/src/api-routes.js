let router = require('express').Router();

router.get('/', function (req, res) {
    res.json({
        status: 'API Its Working',
        message: 'Welcome to RESTHub crafted with love!'
    });
});


const videoController = require('./video/videoController');
router.route('/videos')
    .get(videoController.index)
    .post(videoController.new);

router.route('/videos/:video_id')
    .get(videoController.view)
    .patch(videoController.update)
    .put(videoController.update)
    .delete(videoController.delete);

const appController = require('./app/appController');
router.route('/apps')
    .post(appController.new);
    
router.route('/apps/:appId')
    .get(appController.getManifest)
    .delete(appController.delete);
    



module.exports = router;