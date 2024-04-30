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
        closeBtn.classList.replace("bx-menu", "bx-menu-alt-right");
    } else {
        closeBtn.classList.replace("bx-menu-alt-right", "bx-menu");
    }
}