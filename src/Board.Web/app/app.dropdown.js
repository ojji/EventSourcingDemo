(function($) {
    'use strict';

    class Boarddropdown {
        constructor(element, options) {
            this.$element = element;
            this.options = $.extend({}, Boarddropdown.defaults, this.$element.data, options);
            this._init();
            Foundation.registerPlugin(this, 'Boarddropdown');
        }

        _init() {
            var id = this.$element.attr('id');
            this.$toggler = $(`[data-toggle="${id}"]`);
            this._events();
        }

        _events() {
            this.$element.on("toggle.zf.trigger", this.toggle.bind(this));
        }

        destroy() {
            this.$element.off("toggle.zf.trigger").hide();
            Foundation.unregisterPlugin(this);
        }
        
        toggle() {
            if (this.$element.hasClass('is-open')) {
                this.close();
            } else {
                this.open();
            }
        }

        open() {
            if (this.$element.hasClass('is-open')) {
                return;
            }
            this.$element.addClass('is-open');
            if (this.options.autoClose) {
                this.addBodyHandler();
            }
        }

        addBodyHandler() {
            let $bodyItems = $(document.body).not(this.$element);
            let _this = this;
            $bodyItems.off('click.zf.boarddropdown')
                .on('click.zf.boarddropdown', function (ev) {
                    // if we clicked on the element or on its descendant
                    if (_this.$element.find(ev.target).length) {
                        return;
                    }
                    // if we clicked on the toggler
                    if (_this.$toggler.is(ev.target) || _this.$toggler.find(ev.target).length) {
                        return;
                    }

                    _this.close();
                    $bodyItems.off('click.zf.boarddropdown');
                });
        }

        close() {
            if (!this.$element.hasClass('is-open')) {
                return;
            }
            this.$element.removeClass('is-open');
        }
    }

    Boarddropdown.defaults = {
        autoClose: true
    };

    Foundation.plugin(Boarddropdown, 'Boarddropdown');
})(jQuery);