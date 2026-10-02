// ===== CSRF Token Helper =====
function getToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

// ===== API Helper =====
async function apiPost(url, data) {
    const res = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getToken()
        },
        body: JSON.stringify(data)
    });
    return res.json();
}

// ===== Toast Notification =====
function showToast(message, type = 'success') {
    const id = 'toast_' + Date.now();
    const color = type === 'success' ? '#22c55e' : type === 'danger' ? '#ef4444' : '#f59e0b';
    const icon = type === 'success' ? 'fa-check-circle' : type === 'danger' ? 'fa-times-circle' : 'fa-exclamation-triangle';
    const html = `
    <div id="${id}" style="
        position:fixed; bottom:24px; right:24px; z-index:9999;
        background:#1e293b; border:1px solid ${color}33;
        border-left: 4px solid ${color};
        color:#f8fafc; padding:1rem 1.4rem;
        border-radius:12px; font-size:0.875rem;
        box-shadow:0 8px 32px rgba(0,0,0,0.4);
        display:flex; align-items:center; gap:0.7rem;
        animation: slideInRight 0.3s ease;
        max-width: 360px;
    ">
        <i class="fas ${icon}" style="color:${color};font-size:1.1rem;flex-shrink:0"></i>
        <span>${message}</span>
    </div>`;
    document.body.insertAdjacentHTML('beforeend', html);
    setTimeout(() => { const el = document.getElementById(id); if (el) el.remove(); }, 3500);
}

// ===== Confirmation Dialog =====
function showConfirm(message) {
    return new Promise(resolve => {
        const id = 'confirm_' + Date.now();
        const html = `
        <div id="${id}" style="
            position:fixed;inset:0;background:rgba(0,0,0,0.6);
            display:flex;align-items:center;justify-content:center;z-index:10000;
            backdrop-filter:blur(4px);
        ">
            <div style="
                background:#1e293b;border:1px solid rgba(255,255,255,0.1);
                border-radius:16px;padding:2rem;max-width:380px;width:90%;
                box-shadow:0 24px 80px rgba(0,0,0,0.5);
                animation: fadeInUp 0.25s ease;
            ">
                <div style="text-align:center;margin-bottom:1.5rem;">
                    <div style="width:56px;height:56px;background:rgba(239,68,68,0.15);border-radius:50%;
                        display:flex;align-items:center;justify-content:center;margin:0 auto 1rem;">
                        <i class="fas fa-exclamation-triangle" style="color:#ef4444;font-size:1.4rem;"></i>
                    </div>
                    <h6 style="font-weight:600;margin-bottom:0.5rem;color:#f8fafc;">Xác nhận xóa</h6>
                    <p style="color:#94a3b8;font-size:0.875rem;margin:0;">${message}</p>
                </div>
                <div style="display:flex;gap:0.75rem;">
                    <button onclick="document.getElementById('${id}').remove();resolve(false);"
                        style="flex:1;padding:0.65rem;border:1px solid rgba(255,255,255,0.1);
                        background:transparent;color:#94a3b8;border-radius:8px;cursor:pointer;
                        font-size:0.875rem;transition:all 0.2s;"
                        onmouseover="this.style.background='rgba(255,255,255,0.05)'"
                        onmouseout="this.style.background='transparent'"
                        id="btn_no_${id}">
                        Hủy
                    </button>
                    <button onclick="document.getElementById('${id}').remove();"
                        style="flex:1;padding:0.65rem;border:none;
                        background:#ef4444;color:#fff;border-radius:8px;cursor:pointer;
                        font-size:0.875rem;font-weight:600;transition:all 0.2s;"
                        onmouseover="this.style.background='#dc2626'"
                        onmouseout="this.style.background='#ef4444'"
                        id="btn_yes_${id}">
                        Xóa
                    </button>
                </div>
            </div>
        </div>`;
        document.body.insertAdjacentHTML('beforeend', html);
        document.getElementById(`btn_no_${id}`).addEventListener('click', () => resolve(false));
        document.getElementById(`btn_yes_${id}`).addEventListener('click', () => resolve(true));
    });
}

// ===== Check Label Toggle (Tags) =====
document.addEventListener('click', function (e) {
    const label = e.target.closest('.check-label');
    if (!label) return;
    label.classList.toggle('checked');
    const cb = label.querySelector('input[type="checkbox"]');
    if (cb) cb.checked = label.classList.contains('checked');
});

// ===== Animations =====
const style = document.createElement('style');
style.textContent = `
@keyframes slideInRight {
    from { opacity: 0; transform: translateX(30px); }
    to { opacity: 1; transform: translateX(0); }
}
@keyframes fadeInUp {
    from { opacity: 0; transform: translateY(20px); }
    to { opacity: 1; transform: translateY(0); }
}`;
document.head.appendChild(style);
