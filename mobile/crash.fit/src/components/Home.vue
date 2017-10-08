<template>
  <div></div>
</template>

<script>
export default {
  data () {
    return {
    }
  },
  computed: {
  },
  methods: {
  },
  created () {
    var self = this;
    setInterval(function () {
          if(!self.isLoggedIn){
            self.$router.push({name: 'login'});
          }
      }, 1000); 
  },
  beforeDestroy () {

  }
}
</script>

<style lang="stylus">
</style>
