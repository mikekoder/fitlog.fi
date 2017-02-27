<template>
    <div>
    </div>
</template>

<script>
    var api = require('../api');
    var auth = require('../auth');
module.exports = {
    data () { return {}},
    components: {},
    methods: {},
    created: function () {
        api.getUser().then(function (user) {
            auth.setUserInfo(user.id);
            window.location = '/';
        });
    }
}
</script>

<style scoped>
</style>