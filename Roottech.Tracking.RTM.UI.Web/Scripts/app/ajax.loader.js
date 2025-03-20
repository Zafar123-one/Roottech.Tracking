(function ($) {

    // cool way to find the current element with the highest z-index
    var maxZIndex = Math.max.apply(null, $.map($('body *'), function (e, n) {
        if ($(e).css('position') == 'absolute')
            return parseInt($(e).css('z-index')) || 1;
    })
        );

    // show/hide ajax loader panel methods
    $.fn.showLoaderPanel = function (spinnerSize) {
        spinnerSize = typeof spinnerSize !== 'undefined' ? spinnerSize : false;
        //console.log(spinnerSize);

        var element, elementOffset, activeLoaderClass, panel, panelWidth, panelHeight, spinner;
        element = $(this);
        activeLoaderClass = 'loader-panel-active';
        panel = $('<div class="ddt-loader"></div>');
        spinner = $('<div></div>');
        elementOffset = element.offset();

        // check if there is allready a visible loader panel
        if (element.hasClass(activeLoaderClass)) {
            //console.log('there is allready a active loader panel!');
            return;
        } else {
            // add class indicator
            element.addClass(activeLoaderClass);
            //console.log('generating loader panel..');

            // add parent element indicator
            element.data('panel', panel);
            //console.log(element.data('panel'));

            // determine/set size/position of element
            panelWidth = element.width();
            panelHeight = element.height();
            //console.log('panelWidth: ' + panelWidth);
            //console.log('panelHeight: ' + panelHeight);

            //panel.css('width', panelWidth);
            panel.css('height', panelHeight);
            //panel.css('top', elementOffset.top);
            //panel.css('left', elementOffset.left);
            panel.css('z-index', maxZIndex + 1);

            // position and generate spinner
            spinner.css('display', 'none');
            spinner.css('position', 'absolute');
            //Asif spinner.css('top', panel.offset().top + (panel.height() / 2 - 24));
            //Asif spinner.css('left', panel.offset().left + (panel.width() / 2 - 24));
            spinner.css('z-index', panel.css('z-index') + 1);

            // determine spinner size (sizes 16x16, 32x32, 48x48)
            if (spinnerSize) {
                // override spinner size
                spinner.addClass('ddt-spinner-' + spinnerSize);
            } else if (panelWidth > 400 && panelHeight > 400) {
                // big spinner
                spinner.addClass('ddt-spinner-big');
                //console.log('add big spinner class!');
            } else if (panelWidth > 200 && panelWidth < 400 && panelHeight > 200 && panelHeight < 400) {
                // medium spinner
                spinner.addClass('ddt-spinner-medium');
                //console.log('add medium spinner class!');
            } else if (panelWidth > 20 && panelWidth < 200 && panelHeight > 20 && panelHeight < 200) {
                // small spinner
                spinner.addClass('ddt-spinner-small');
                //console.log('add small spinner class!');
            } else if (panelWidth > 20 && panelWidth < 250 && panelHeight > 15 && panelHeight < 200) {
                // small spinner
                spinner.addClass('ddt-spinner-small');
                //console.log('add small spinner class!');
                spinner.css('top', 2);
                spinner.css('left', -9);
                panel.css('top', elementOffset.top + 2);
                panel.css('z-index', $(".ui-dialog #featureList").parent().css("z-index") + 1);
            }



            // generate loader panel
            $(this).append(panel);

            // fade in loader panel
            panel.fadeIn(500, function () {
                $(this).append(spinner);
                spinner.show();
            });
        }
    };

    $.fn.hideLoaderPanel = function () {
        var activeLoaderClass = 'loader-panel-active';
        var element = $(this);

        // remove class indicator
        element.removeClass(activeLoaderClass);

        if (element.data('panel')) {
            // fadeout and remove loader panel from element
            element.data('panel').fadeOut();
        }
    };

})(jQuery);