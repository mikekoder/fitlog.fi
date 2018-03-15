<template>
    <q-page class="q-pa-sm">

          <div class="row">
            <q-btn glossy color="primary" v-if="type == 'Improvement'" :label="$t('askImprovement')" @click="createFeedback"></q-btn>
            <q-btn glossy color="primary" v-if="type == 'Bug'" :label="$t('reportBug')" @click="createFeedback"></q-btn>
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
</template>

<script src="./feedback.js">
</script>

<style scoped>
</style>