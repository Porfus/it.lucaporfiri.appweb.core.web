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