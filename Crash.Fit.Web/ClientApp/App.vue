<template>
    <div class="container-fluid">
        <div class="row">
            <div class="col-sm-2">
              <mainmenu></mainmenu>
            </div>
            <div class="col-sm-10">
                <transition name="md-router">
                    <router-view></router-view>
                </transition>
            </div>
        </div>
    </div>
</template>

<script>
//import Toolbar from './components/Toolbar'
module.exports =  {
    name: 'App',
    data: function () {
        return { }
    },
    components: {
        'mainmenu': require('./components/menu')
    }
}
</script>

<style>
</style>