<template>
<layout >

  <span slot="title" v-if="type == 'Improvement'" >
      {{ $t('improvements') }}
  </span>
  <span slot="title" v-if="type == 'Bug'" >
       {{ $t('bugs') }}
  </span>

  <div slot="toolbar">
    <q-btn flat icon="fa-plus" v-if="type == 'Improvement'" :label="$t('improvement')" @click="createFeedback"></q-btn>
    <q-btn flat icon="fa-plus" v-if="type == 'Bug'" :label="$t('bug')" @click="createFeedback"></q-btn>
  </div>
    <q-page class="q-pa-sm">

          <div class="row">
            
          </div>
        <q-card v-for="item in itemsSorted" @click.native="showFeedback(item)" class="q-mt-sm">
            <q-card-title :class="cardTitleBackground">
                {{ item.title }}
            </q-card-title>
            <q-card-separator />
            <q-card-main>
                {{ item.description }}
                <div class="row" v-if="item.adminComment">
                    <div class="col">
                        <pre class="bg-light-blue-1 q-pa-md">{{ item.adminComment }}</pre>
                    </div>
                </div>
            </q-card-main>
            <q-card-actions>
                <q-btn color="primary" icon="fa-thumbs-up" glossy @click="vote(item)" :label="item.score" :disabled="userHasVoted(item.id)" :title="$t('youHaveVoted')"></q-btn>

            </q-card-actions>
        </q-card>
    </q-page>
    </layout>
</template>

<script src="./feedback.js">
</script>

<style scoped>
</style>