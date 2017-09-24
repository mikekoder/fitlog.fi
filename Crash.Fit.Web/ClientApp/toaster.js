import 'bootstrap-notify'

export default {
    error(content, duration) {
        $.notify({
            message: content,
            icon: 'glyphicon glyphicon-warning-sign',
        }, {
            type: 'danger',
            delay: duration || 10000,
            placement: {
                from: "bottom",
                align: "center"
            },
        });
    },
    info(content, duration){
        $.notify({
            message: content,
            icon: 'glyphicon glyphicon-warning-sign',
        }, {
            type: 'info',
            delay: duration || 10000,
            placement: {
                from: "bottom",
                align: "center"
            },
        });
    }
}
