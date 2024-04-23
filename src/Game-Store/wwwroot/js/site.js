// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let sidebar = document.querySelector(".sidebar");
let closeBtn = document.querySelector("#btn");

sidebar.addEventListener("mouseenter", function () {
    sidebar.classList.toggle("open");
    menuBtnChange();
});

sidebar.addEventListener("mouseleave", function () {
    sidebar.classList.toggle("open");
    menuBtnChange();
});

function menuBtnChange() {
    if (sidebar.classList.contains("open")) {
        closeBtn.classList.replace("bx-menu", "bx-menu-alt-right");//replacing the iocns class
    } else {
        closeBtn.classList.replace("bx-menu-alt-right", "bx-menu");//replacing the iocns class
    }
}