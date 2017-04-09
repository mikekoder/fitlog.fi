<template>
    <select v-for="musclegroup in musclegroups">
        <option>{{ musclegroup.name }}</option>
    </select>
</template>

<script>
    var api = require('../../api');
    require('bootstrap-3-typeahead');

module.exports = {
    data: function() {
        return {
        }
    },
    props:{
        value: {},
        muscleGroups: null
    },
    mounted: function () {
    }
}
</script>