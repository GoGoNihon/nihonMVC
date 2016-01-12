


    function playYoutubeVideo(videoID, playerID) {

        var player2 = new YT.Player(playerID, {
            videoId: videoID,
            playerVars: { 'loop': 0, 'controls': 1, 'rel': 0, 'autoplay': 0, 'showinfo': 0 },
            events: {
                'onReady': onNewVideoReady,
                'onStateChange': onNewPlayerStateChange
            }

        });
    }

    function onNewVideoReady(event) {
        //event.target.playVideo();
    }

    function onNewPlayerStateChange(event) {
        

    }


    var videoFunction;
    var tag = document.createElement('script');

    tag.src = "https://www.youtube.com/iframe_api";
    var firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

    // 3. This function creates an <iframe> (and YouTube player)
    //    after the API code downloads.
    // player;
    var id = "9IV3aEMls2g";
    var start = 12;
    var end = 40;
    function onYouTubeIframeAPIReady() {

      videoFunction =  function asdf() { alert(); };

        player = new YT.Player('player', {
            videoId: id,
            height: 100,
            playerVars: { 'start': start, 'end': end, 'loop': 1, 'controls': 0, 'rel': 0, 'autoplay': 0, 'showinfo': 0 },
            events: {
                'onReady': onPlayerReady,
                'onStateChange': onPlayerStateChange
            }

        });
    }

    // 4. The API will call this function when the video player is ready.
    function onPlayerReady(event) {

        var a = $("#video-mask");
        if (($(window).scrollTop() + $(window).height()) >= a.offset().top) {
            event.target.playVideo();
        }

        //event.target.playVideo();
        event.target.mute();
        //event.target.setSize(1920,1080);
    }

    // 5. The API calls this function when the player's state changes.
    //    The function indicates that when playing a video (state=1),
    //    the player should play for six seconds and then stop.
    var done = false;
    function onPlayerStateChange(event) {
        if (event.data == YT.PlayerState.ENDED) {
            player.loadVideoById({
                videoId: id,
                startSeconds: start,
                endSeconds: end
            })

        }
    }
    function stopVideo() {
        //alert();
        player.stopVideo();
    }
