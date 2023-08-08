$(async function () {
    let loginInfo = await app.checkLogin();
    console.log(loginInfo);
    $('#title').text(`Xin chào ${loginInfo?.data?.guardian?.name} - ${loginInfo?.data?.guardian?.phone}`);
})