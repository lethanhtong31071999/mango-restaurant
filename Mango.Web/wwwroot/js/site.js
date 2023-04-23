// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('header .navbar-nav').find(`[href="${window.location.pathname}"]`).addClass('active');

    // Side bar
    const activeElement = $('.sidebar .navbar-nav').find(`[href="${window.location.pathname}"]`);
    activeElement.addClass('active');
    if (activeElement.hasClass('child-box')) {
        let currentParent = activeElement.parent()
        while (currentParent !== $('.sidebar')) {
            if (currentParent.hasClass('container-management')) {
                const navLinkEl = currentParent.find('.nav-link.dropdown-toggle[data-bs-toggle="dropdown"]')
                navLinkEl.addClass('active');
                navLinkEl.addClass('show')
                navLinkEl.attr('aria-expanded', true)
                break;
            }
            if (currentParent.hasClass('list-management')) {
                currentParent.addClass('show');
            }
            currentParent = currentParent.parent();
        }
    }
});

