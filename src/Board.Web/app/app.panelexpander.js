(function($) {
    $(document).on('click.zf.trigger', '[data-minimize]', function() {
        $(this).trigger('minimize.board.trigger');
    });

    $(document).on('minimize.board.trigger', '[data-minimizable]', function(e) {
        e.stopPropagation();
        let element = e.currentTarget;
        $(element).toggleClass('minimized');
    });
})(jQuery);