// ===== Tema claro / oscuro =====
// El tema inicial lo aplica un script en el <head> del layout. Aca solo se maneja el boton.
(function () {
    var boton = document.getElementById('themeToggle');
    if (!boton) return;

    function actualizarIcono() {
        var oscuro = document.documentElement.getAttribute('data-bs-theme') === 'dark';
        // Muestra a que tema se va a pasar al hacer clic.
        boton.textContent = oscuro ? '☀' : '☾';
        boton.title = oscuro ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro';
    }

    boton.addEventListener('click', function () {
        var nuevo = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-bs-theme', nuevo);
        try { localStorage.setItem('qz-tema', nuevo); } catch (e) { /* sin localStorage: vale solo para esta pagina */ }
        actualizarIcono();
    });

    actualizarIcono();
})();

// Los formularios con data-loading="texto" deshabilitan su boton y muestran un spinner al enviarse.
// Sirve para "Generar preguntas": la IA tarda unos segundos y sin esto parece que no paso nada.
document.addEventListener('submit', function (e) {
    var form = e.target;
    var texto = form.getAttribute && form.getAttribute('data-loading');
    // Si la validacion del formulario cancelo el envio, no se hace nada.
    if (!texto || e.defaultPrevented) return;

    var boton = form.querySelector('button[type=submit]');
    if (!boton) return;

    // En el siguiente ciclo: asi el envio no se ve afectado por deshabilitar el boton.
    setTimeout(function () {
        boton.disabled = true;
        boton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>' + texto;
    }, 0);
});
