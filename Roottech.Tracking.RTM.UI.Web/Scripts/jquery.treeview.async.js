/*
 * Async Treeview 0.1 - Lazy-loading extension for Treeview
 *
 * http://bassistance.de/jquery-plugins/jquery-plugin-treeview/
 *
 * Copyright (c) 2007 Jörn Zaefferer
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Revision: $Id$
 *
 */

;(function($) {

function load(settings, root, child, container) {
	function createNode(parent) {
	    if  (!this.hasChildren && this.fullName == null) return; // when there is no child node and not last node don't show.
		var current = $("<li/>").attr("id", this.id || "").html("<span " + (root == "source" ? "class='bold'" : "") + ">" + this.text + "</span>").appendTo(parent);
		if (this.classes) {
			current.children("span").addClass(this.classes);
		}
		if (this.expanded) {
			current.addClass("open");
		}
	    ///Changes by asif on 10/09/2012 for document attachment in carer advance and placement
        if (!this.hasChildren && this.fullName != null) {
            var text = $(current).children("span").text();
            $(current).children("span").text("");
            
            $(current).children("span")
                .prepend("<table>" +
                            "<tr> " +
                                "<td><input type='checkbox' name=" + this.id + " id='chkAsset" + this.id + "' unitid='" + this.fullName + "' /></td>" +
                                "<td><img id='imgIgnition"+ this.id  +"' src='../Images/vehicleIcons/acc_off.png'></img></td>" +
                                "<td><img id='imgSignal"+ this.id  +"' src='../Images/vehicleIcons/gsmOff.png'></img></td>" +
                                "<td><img id='imgAlert"+ this.id  +"' src='../Images/vehicleIcons/alarm_off.png'></img></td>" +
                                "<td><label>" + text + "</label></td>" +
                            "</tr>" +
                         "</table>" );
            
            $("#chkAsset" + this.id).click(function() {
                toggleAsset(this);
            });
        }

	    //console.log(this.fullName);
	    ///
		if (this.hasChildren || this.children && this.children.length) {
			var branch = $("<ul/>").appendTo(current);
			if (this.hasChildren) {
				current.addClass("hasChildren");
				createNode.call({
					classes: "placeholder",
					text: "&nbsp;",
					children:[]
				}, branch);
			}
			if (this.children && this.children.length) {
			    $.each(this.children, createNode, [branch]);
			}
		}
	}
	$.ajax($.extend(true, {
		url: settings.url,
		dataType: "json",
		data: {
			root: root
		},
		success: function(response) {
			child.empty();
			$.each(response, createNode, [child]);
	        $(container).treeview({add: child});
	    }
	}, settings.ajax));
	/*
	$.getJSON(settings.url, {root: root}, function(response) {
		function createNode(parent) {
			var current = $("<li/>").attr("id", this.id || "").html("<span>" + this.text + "</span>").appendTo(parent);
			if (this.classes) {
				current.children("span").addClass(this.classes);
			}
			if (this.expanded) {
				current.addClass("open");
			}
			if (this.hasChildren || this.children && this.children.length) {
				var branch = $("<ul/>").appendTo(current);
				if (this.hasChildren) {
					current.addClass("hasChildren");
					createNode.call({
						classes: "placeholder",
						text: "&nbsp;",
						children:[]
					}, branch);
				}
				if (this.children && this.children.length) {
					$.each(this.children, createNode, [branch])
				}
			}
		}
		child.empty();
		$.each(response, createNode, [child]);
        $(container).treeview({add: child});
    });
    */
}

var proxied = $.fn.treeview;
$.fn.treeview = function(settings) {
	if (!settings.url) {
		return proxied.apply(this, arguments);
	}
	if (!settings.root) {
		settings.root = "source";
	}
	var container = this;
	if (!container.children().size())
		load(settings, settings.root, this, container);
	var userToggle = settings.toggle;
	return proxied.call(this, $.extend({}, settings, {
		collapsed: true,
		toggle: function() {
			var $this = $(this);
			if ($this.hasClass("hasChildren")) {
				var childList = $this.removeClass("hasChildren").find("ul");
				load(settings, this.id, childList, container);
			}
			if (userToggle) {
				userToggle.apply(this, arguments);
			}
		}
	}));
};

})(jQuery);
