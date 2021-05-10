Video = require('./videoModel');

exports.index = (req, res) => {
    Video.get( (err, videos) => {
        if (err) {
            res.json({ status: 'error', message: err });
        }

        res.json({ status: 'sucess', message: 'Video retrieved successfully', data: videos});
    });
}

exports.new = (req, res) => {
    const video = new Video();
    video.name = req.body.name ? req.body.name : video.name;
    
    video.save( err => {
        if (err) res.json(err);

        res.json({ message: 'New video created!', data: video});
    })
}

exports.view = (req, res) => {
    Video.findById(req.params.video_id, (err, video) => {
        if (err) res.send(err);

        res.json({message: 'Video details loading..', data: video});
    })
}

exports.update = (req, res) => {
    Video.findById(req.params.video_id, (err, video) => {
        if (err) err.send(err);

        contact.name = req.body.name ? req.body.name : contact.name;

        video.save( err => {
            if (err) res.json(err);

            res.json({message: 'Video info updated', data: video});
        });
    });
}

exports.delete = (req, res) => {
    Video.remove({_id: req.params.video_id}, (err, video) => {
        if (err) res.json(err);

        res.json({ status: 'success', message: 'Video deleted'});
    });
}