(function () {
  var el = document.getElementById('admin-now');
  if (!el) return;

  var utc = el.getAttribute('data-utc');
  if (!utc) return;

  // Initial UTC value from the server
  var startUtc = new Date(utc).getTime();
  // When the script started (client timestamp)
  var mountedAt = Date.now();

  // Format similar to .NET "f" (Long date + short time, without seconds)
  function formatLikeF(d) {
    try {
      return d.toLocaleString(undefined, {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch {
      return d.toLocaleString();
    }
  }

  function render() {
    // How much time has passed on the client since mount
    var elapsed = Date.now() - mountedAt;
    // "Now" = server UTC + elapsed time, displayed in the admin's local zone
    var nowLocal = new Date(startUtc + elapsed);
    el.textContent = formatLikeF(nowLocal);
  }

  // First update immediately
  render();

  // Update every minute (enough since we don’t show seconds)
  var timer = setInterval(render, 60 * 1000);

  // Cleanup (best practice)
  window.addEventListener('unload', function () {
    clearInterval(timer);
  });
})();
