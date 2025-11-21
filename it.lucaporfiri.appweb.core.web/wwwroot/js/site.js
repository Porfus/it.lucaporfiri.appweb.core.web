// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Funzione per il Theme Switcher
document.addEventListener('DOMContentLoaded', function () {
    const themeSwitcher = document.getElementById('theme-switcher');
    const htmlElement = document.documentElement; // Seleziona l'elemento <html>

    // Controlla se un tema è già stato salvato in localStorage
    const currentTheme = localStorage.getItem('theme');
    if (currentTheme) {
        htmlElement.setAttribute('data-theme', currentTheme);
    }

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

    // --- SELEZIONE ELEMENTI ---
    const body = document.body;
    const sidebarToggler = document.getElementById('sidebarToggler'); // Freccia Desktop
    const mobileToggle = document.getElementById('mobileToggle');     // Hamburger Mobile
    const sidebar = document.getElementById('sidebar');

    // --- LOGICA STATO COLLASSATO (Desktop) ---

    // 1. Controlla se c'è una preferenza salvata nel LocalStorage
    // Questo serve per "ricordare" se l'utente aveva chiuso la sidebar anche dopo aver ricaricato la pagina.
    const isCollapsed = localStorage.getItem('sidebar-collapsed') === 'true';

    if (isCollapsed) {
        body.classList.add('sidebar-collapsed');
    }

    // 2. Gestione Click sul Toggler Desktop
    if (sidebarToggler) {
        sidebarToggler.addEventListener('click', function () {
            // Aggiunge/Rimuove la classe al body
            body.classList.toggle('sidebar-collapsed');

            // Salva la preferenza dell'utente
            const isNowCollapsed = body.classList.contains('sidebar-collapsed');
            localStorage.setItem('sidebar-collapsed', isNowCollapsed);
        });
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
    // Se l'utente clicca sul contenuto principale mentre la sidebar è aperta su mobile, chiudila.
    document.addEventListener('click', function (e) {
        // Se la sidebar è aperta su mobile...
        if (body.classList.contains('mobile-sidebar-active')) {
            // ...e il click NON è avvenuto dentro la sidebar e NON è sul pulsante hamburger...
            if (!sidebar.contains(e.target) && !mobileToggle.contains(e.target)) {
                // ...chiudi la sidebar.
                body.classList.remove('mobile-sidebar-active');
            }
        }
    });

});