// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Funzione per il Theme Switcher
document.addEventListener('DOMContentLoaded', function () {
    const themeSwitcher = document.getElementById('theme-switcher');
    const htmlElement = document.documentElement; 

    if (themeSwitcher) {
        themeSwitcher.addEventListener('click', function () {
            // Controlla qual è il tema attuale e impostalo sull'opposto
            if (htmlElement.getAttribute('data-theme') === 'dark') {
                htmlElement.setAttribute('data-theme', 'light');
                localStorage.setItem('theme', 'light');
            } else {
                htmlElement.setAttribute('data-theme', 'dark');
                localStorage.setItem('theme', 'dark');
            }
        });
    }
});

//Funzione per la Sidebar Collassabile e Mobile
document.addEventListener("DOMContentLoaded", function () {

    
    const body = document.body;
    const sidebarToggler = document.getElementById('sidebarToggler');
    const mobileToggle = document.getElementById('mobileToggle');     
    const sidebar = document.getElementById('sidebar');

    // --- LOGICA STATO COLLASSATO (Desktop) ---
    if (sidebarToggler) {
        const togglerIcon = sidebarToggler.querySelector('i');
        sidebarToggler.addEventListener('click', function () {
            
            body.classList.toggle('sidebar-collapsed');

            
            const isNowCollapsed = body.classList.contains('sidebar-collapsed');
            localStorage.setItem('sidebar-collapsed', isNowCollapsed);

            
            if (togglerIcon) {
                togglerIcon.classList.toggle('rotated', isNowCollapsed);
            }
        });

        
        const isCollapsed = body.classList.contains('sidebar-collapsed');
        if (togglerIcon) {
            togglerIcon.classList.toggle('rotated', isCollapsed);
        }
    }


    // --- LOGICA MOBILE (Hamburger Menu) ---

    if (mobileToggle) {
        mobileToggle.addEventListener('click', function (e) {
            // Ferma la propagazione per evitare che il click venga catturato dal listener "clicca fuori"
            e.stopPropagation();
            body.classList.toggle('mobile-sidebar-active');
        });
    }

    // --- CHIUSURA SIDEBAR CLICCANDO FUORI (Mobile UX) ---
    document.addEventListener('click', function (e) {
        
        if (body.classList.contains('mobile-sidebar-active')) {
        
            if (!sidebar.contains(e.target) && !mobileToggle.contains(e.target)) {
        
                body.classList.remove('mobile-sidebar-active');
            }
        }
    });

});