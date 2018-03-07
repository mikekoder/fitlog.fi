<template>
  <q-page class="q-pa-sm">
      <q-scroll-area>
          <q-list v-if="recipes.length > 0">
              <q-item v-for="(recipe,index) in recipes" @click="showRecipe(recipe)" :key="index" :separator="true">{{ recipe.name }}</q-item>
          </q-list>
          <div v-else>{{ $t('noRecipes') }}</div>
      </q-scroll-area>
    <div class="row q-my-lg">
      <q-btn color="primary" glossy  icon="fa-plus" @click="createRecipe" :label="$t('recipe')"></q-btn>
    </div>
  </q-page>
</template>

<script src="./recipes.js">
</script>

<style lang="stylus">
/*
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
*/
</style>
