<template>
  <div>Exercises</div>
</template>

<script>
import constants from '../store/constants'

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
    self.$store.commit(constants.LOADING_DONE);
  },
  beforeDestroy () {

  }
}
</script>

<style lang="stylus">
</style>
