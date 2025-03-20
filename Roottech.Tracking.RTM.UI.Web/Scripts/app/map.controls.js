var t = '';
// The HomeControl adds a control to the map that returns the user to the control's defined home.

function HomeControl(map, home, index) {

    // Get the control DIV. We'll attach our control UI to this DIV.
    var controlDiv = document.createElement('div');
    controlDiv.index = index;

    // We set up a variable for the 'this' keyword since we're adding event
    // listeners later and 'this' will be out of scope.
    var control = this;

    // Set the home property upon construction.
    control.home_ = home;

    // Set CSS styles for the DIV containing the control. Setting padding to
    // 5 px will offset the control from the edge of the map.
    controlDiv.style.padding = '5px';

    // Set CSS for the goHome control border.
    var goHomeUi = createControlDiv("Click to set the map to Home", "Home");
    //controlDiv.appendChild(goHomeUi);

    // Set CSS for the setHome control border.
    var setHomeUi = createControlDiv("Click to set Home to the current center", "Set Home");
   // controlDiv.appendChild(setHomeUi);
    
    // Set CSS for the logout control border.
    var logout = createControlDiv("Click to logout", "Logout");
   // controlDiv.appendChild(logout);
    
    // Setup the click event listener for Home:
    // simply set the map to the control's current home property.
    google.maps.event.addDomListener(goHomeUi, 'click', function () {
        var currentHome = control.getHome();
        map.setCenter(currentHome);
    });

    // Setup the click event listener for Set Home:
    // Set the control's home to the current Map center.
    google.maps.event.addDomListener(setHomeUi, 'click', function () {
        var newHome = map.getCenter();
        control.setHome(newHome);
    });
    
    // Setup the click event listener for Set Home:
    // Set the control's home to the current Map center.
    google.maps.event.addDomListener(logout, 'click', function () {
        
    });
    /*
    var div = document.createElement("div");
    div.id = "hand_b";
    div.class = "selected";
    div.onclick = stopEditing();
    controlDiv.appendChild(div);

    div = document.createElement("div");
    div.id = "placemark_b";
    div.class = "unselected";
    div.onclick = placeMarker();
    controlDiv.appendChild(div);

    div = document.createElement("div");
    div.id = "line_b";
    div.class = "unselected";
    div.onclick = startLine();
    controlDiv.appendChild(div);

    div = document.createElement("div");
    div.id = "shape_b";
    div.class = "unselected";
    div.onclick = startShape();
    controlDiv.appendChild(div);*/
    
    return controlDiv;
}

function createControlDiv(title, text) {
    var div = document.createElement('div');
    div.style.backgroundColor = 'rgba(246, 249, 252, 0.8)';
    div.style.borderStyle = 'solid';
    div.style.borderWidth = '2px';
    div.style.cursor = 'pointer';
    div.style.textAlign = 'center';
    div.title = title;
    
    var divText = document.createElement('div');
    divText.style.fontFamily = 'Arial,sans-serif';
    divText.style.fontSize = '12px';
    divText.style.paddingLeft = '4px';
    divText.style.paddingRight = '4px';
    divText.innerHTML = '<strong>' + text + '</strong>';
    div.appendChild(divText);

    return div;
}

// Define a property to hold the Home state.
HomeControl.prototype.home_ = null;

// Define setters and getters for this property.
HomeControl.prototype.getHome = function() {
    return this.home_;
};

HomeControl.prototype.setHome = function(home) {
    this.home_ = home;
};