$(".btn-loader").on("click", function () {
    var $btn = $(this);
    var btnText = $btn.text();
    $btn.addClass("disabled");
    $btn.html('<i class="spinner-border mx-2" style="width: 1rem; height: 1rem;"></i>' + btnText);
    RemoveBtnLoader($btn, btnText);
});

$(".btn-loader-Server").on("click", function () {
    let $btn = $(this);
    var btnData = addBtnLoader($btn);
    RemoveBtnLoader(btnData.btnRef, btnData.oldbtnText);
});

function addBtnLoader(btnRef) {
    let oldbtnText = btnRef.text();
    btnRef.addClass("disabled");
    btnRef.html('<i class="spinner-border mx-2" style="width: 1rem; height: 1rem;"></i>' + oldbtnText);
    return { btnRef, oldbtnText }
}

function RemoveBtnLoader(btnRef, oldbtnText) {
    setTimeout(function () {
        btnRef.removeClass("disabled");
        btnRef.html(oldbtnText);
    }, 500);
}

$('#darkmood').on("change", function () {
    if ($(this).is(':checked')) {
        $('html').attr('data-bs-theme', 'dark');
    } else {
        $('html').attr('data-bs-theme', 'light');

    }
});
