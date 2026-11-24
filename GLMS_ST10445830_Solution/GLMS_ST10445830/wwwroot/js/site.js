// GLMS – Play theme behaviour (ported from Play Astro scripts)
document.addEventListener('DOMContentLoaded', function () {
    var root = document.documentElement;

    // ===== Dark mode switcher
    var themeSwitcher = document.getElementById('themeSwitcher');
    if (themeSwitcher) {
        themeSwitcher.addEventListener('click', function () {
            var next = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
            root.setAttribute('data-bs-theme', next);
            localStorage.setItem('theme', next);
        });
    }

    // ===== Mobile menu toggler
    var toggler = document.getElementById('navbarToggler');
    var collapse = document.getElementById('navbarCollapse');
    if (toggler && collapse) {
        toggler.addEventListener('click', function () {
            toggler.classList.toggle('navbarTogglerActive');
            collapse.classList.toggle('show');
        });
    }

    // ===== Sticky header shadow + back-to-top button
    var header = document.querySelector('.ud-header');
    var backToTop = document.querySelector('.back-to-top');
    var onScroll = function () {
        var y = window.pageYOffset || root.scrollTop;
        if (header) header.classList.toggle('sticky-header', y > 0);
        if (backToTop) backToTop.style.display = y > 50 ? 'flex' : 'none';
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();

    if (backToTop) {
        backToTop.addEventListener('click', function (e) {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }
});
