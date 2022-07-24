document.addEventListener("DOMContentLoaded", function (event) {

    const showNavbar = (toggleId, navId, bodyId, headerId,anchorId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId),
            anchorpd = document.getElementById(anchorId)

        // Validate that all variables exist
        if (toggle && nav && bodypd && headerpd) {
            toggle.addEventListener('click', () => {
                // show navbar
                nav.classList.toggle('show_')
                // change icon
                //if ($("#toggle-affect1"))
                toggle.classList.toggle('bx-right-arrow-alt')
                toggle.classList.toggle('bx-arrow-back')
                anchorpd.classList.toggle('align-left')
                anchorpd.classList.toggle('align-right')
                // add padding to body
                bodypd.classList.toggle('body-menu_close')
                bodypd.classList.toggle('body-menu_open')
                // add padding to header
                
            })
        }
    }

    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header','toggle_affect_anchor')

    /*===== LINK ACTIVE =====*/
    const linkColor = document.querySelectorAll('.nav_link')

    function colorLink() {
        if (linkColor) {
            linkColor.forEach(l => l.classList.remove('active'))
            this.classList.add('active')
        }
    }
    linkColor.forEach(l => l.addEventListener('click', colorLink))

});
function imageToggle() {
    const toggleList = document.getElementById("toggleList"),
        toggleDropDown = document.getElementById("toggleDropDown")
    navbarDropdownMenuLink = document.getElementById("navbarDropdownMenuLink")
    toggleList.classList.toggle('show_')
    toggleDropDown.classList.toggle('show_')
    if ($("#navbarDropdownMenuLink").attr('aria-expanded') == "true") {
        $("#navbarDropdownMenuLink").attr('aria-expanded', "false")
    } else {
        $("#navbarDropdownMenuLink").attr('aria-expanded', "true")
    }
}