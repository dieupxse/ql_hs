$(async function () {
    await app.checkLogin();
    $('#dang-nhap-form').submit(async function (e) {
        e.preventDefault();
        let username = $('#username').val();
        let password = $('#password').val();
        if (username == '' || password == '') {
            alert('Tên đăng nhập và mật khẩu không được để trống');
            if ($('#username').val() == '') $('#username').focus();
            if ($('#password').val() == '') $('#password').focus();
            return;
        }
        let result = await app.api.post('/authenticate/login', {
            "Username": username,
            "Password": password
        });
        if (result && result.token) {
            localStorage.setItem(app.api.tokenKey, result.token);
            await app.checkLogin();
        } else {
            alert('Thông tin đăng nhập không chính xác');
        }
    });
})