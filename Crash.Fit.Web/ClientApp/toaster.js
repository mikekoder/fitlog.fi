import 'bootstrap-notify'


function error(content) {
    $.notify({
        message: content,
        icon: 'glyphicon glyphicon-warning-sign',
    }, {
        type: 'danger',
        placement: {
            from: "bottom",
            align: "center"
        },
    });
}
function deleted(text, restoreUrl){

}


export { error }