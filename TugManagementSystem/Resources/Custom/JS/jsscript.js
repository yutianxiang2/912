var css;
function include_css(css_file) {
    var html_doc = document.getElementsByTagName('head')[0];
    css = document.createElement('link');
    css.setAttribute('rel', 'stylesheet');
    css.setAttribute('type', 'text/css');
    css.setAttribute('href', css_file);
    html_doc.appendChild(css);

    // alert state change
    css.onreadystatechange = function () {
        if (css.readyState == 'complete') {
            alert('CSS onreadystatechange fired');
        }
    }
    css.onload = function () {
        alert('CSS onload fired');
        window.location.reload();
    }

    return false;
}


var js;
function include_js(file) {
    var html_doc = document.getElementsByTagName('head')[0];
    js = document.createElement('script');
    js.setAttribute('type', 'text/javascript');
    js.setAttribute('src', file);
    html_doc.appendChild(js);

    js.onreadystatechange = function () {
        if (js.readyState == 'complete') {
            alert('JS onreadystate fired');
        }
    }

    js.onload = function () {
        alert('JS onload fired');
        window.location.reload();
    }

    return false;
}