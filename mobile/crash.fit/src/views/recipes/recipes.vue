<template>
  <div :class="{desktop: isDesktop }">
      <q-scroll-area>
          <q-list>
              <q-item v-for="(recipe,index) in recipes" @click="showRecipe(recipe)" :key="index" :separator="true">{{ recipe.name }}</q-item>
          </q-list>
      </q-scroll-area>
    <div class="row pad">
      <q-btn color="primary" glossy  icon="fa-plus" @click="createRecipe">{{ $t('recipe') }}</q-btn>
    </div>
  </div>
</template>

<script src="./recipes.js">
</script>

<style lang="stylus">
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
</style>
