
function enableButton(button) {

    button.removeAttribute('disabled');
}


function disableButton(button) {

    button.disabled = true;
}

function onSortClick(id) {

    let element = document.getElementById(id);
    let className = element.className;

    if (className == 'oi oi-sort-descending') {
        element.className = 'oi oi-sort-ascending';
    }
    else {
        element.className = 'oi oi-sort-descending'
    }
    
}