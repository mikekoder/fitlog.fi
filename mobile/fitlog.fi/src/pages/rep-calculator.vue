<template>
  <q-page class="q-pa-sm">
    <div class="row">
        <div class="col-3"><q-input v-model="reps" type="number" :float-label="$t('reps')" /></div>
        <div class="col-1 q-pa-md q-px-lg">x</div>
        <div class="col-3"><q-input v-model="weights" type="number" :float-label="$t('weights')" /></div>
        <div class="col-3 q-pa-sm">
          <q-btn glossy color="primary" @click="calculate" :label="$t('calculate')"></q-btn>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12">
            <table class="table">
                <thead>
                    <tr>
                        <th>{{ $t('reps') }}</th>
                        <th>{{ $t('estimate') }}</th>
                        <th>Epley<sub>*</sub></th>
                        <th>Brzycki<sub>*</sub></th>
                        <th>Lander<sub>*</sub></th>
                        <th>Lombardi<sub>*</sub></th>
                        <th>Mayhew<sub>*</sub></th>
                        <th>OConner<sub>*</sub></th>
                        <th>Wathan<sub>*</sub></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="result in results">
                        <td>{{ result.reps }}</td>
                        <td>{{ decimal(result.avg, 2) }}</td>
                        <td>{{ decimal(result.Epley, 2) }}</td>
                        <td>{{ decimal(result.Brzycki, 2) }}</td>
                        <td>{{ decimal(result.Lander, 2) }}</td>
                        <td>{{ decimal(result.Lombardi, 2) }}</td>
                        <td>{{ decimal(result.Mayhew, 2) }}</td>
                        <td>{{ decimal(result.OConner, 2) }}</td>
                        <td>{{ decimal(result.Wathan, 2) }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
  </q-page>
</template>

<script>
import constants from '../store/constants'
export default {
  data () {
    return {
      reps: undefined,
                weights: undefined,
                results: []
    }
  },
  computed: {
  },
  methods: {
    decimal(value, precision) {
                if (!value) {
                    return value;
                }
                return value.toFixed(precision);
            },
            calculate() {
                var r = this.reps;
                if (typeof (r) !== 'number') {
                    r = parseInt(r);
                }
                var w = this.weights;
                if (typeof (w) !== 'number') {
                    w = parseFloat(w.replace(',', '.'));
                }

                if (r > 0 && w > 0) {
                    var e = Epley(r, w);
                    var b = Brzycki(r, w);
                    var la = Lander(r, w);
                    var lo = Lombardi(r, w);
                    var m = Mayhew(r, w);
                    var o = OConner(r, w);
                    var wa = Wathan(r, w);
                    var max = (e + b + la + lo + m + o + wa) / 7;

                    this.results = [];
                    if (r === 1) {
                        this.results.push({
                            reps: 1,
                            avg: w,
                            Epley: w,
                            Brzycki: w,
                            Lander: w,
                            Lombardi: w,
                            Mayhew: w,
                            OConner: w,
                            Wathan: w
                        });
                    }
                    else {
                        this.results.push({
                            reps: 1,
                            avg: max,
                            Epley: e,
                            Brzycki: b,
                            Lander: la,
                            Lombardi: lo,
                            Mayhew: m,
                            OConner: o,
                            Wathan: wa
                        });
                    }
                    for (var reps = 2; reps <= 10; reps++) {
                        if (reps == r) {
                            this.results.push({
                                reps: reps,
                                avg: w,
                                Epley: w,
                                Brzycki: w,
                                Lander: w,
                                Lombardi: w,
                                Mayhew: w,
                                OConner: w,
                                Wathan: w
                            });
                            continue;
                        }
                        var e2 = EpleyReverse(reps, e);
                        var b2 = BrzyckiReverse(reps, b);
                        var la2 = LanderReverse(reps, la);
                        var lo2 = LombardiReverse(reps, lo);
                        var m2 = MayhewReverse(reps, m);
                        var o2 = OConnerReverse(reps, o);
                        var wa2 = WathanReverse(reps, wa);
                        var avg = (e2 + b2 + la2 + lo2 + m2 + o2 + wa2) / 7;

                        this.results.push({
                            reps: reps,
                            avg: avg,
                            Epley: e2,
                            Brzycki: b2,
                            Lander: la2,
                            Lombardi: lo2,
                            Mayhew: m2,
                            OConner: o2,
                            Wathan: wa2
                        });
                    }
                }
            }
  },
  created () {
      var self = this;
      self.$store.commit(constants.LOADING_DONE);
  },
  mounted(){

  },
  beforeDestroy () {

  }
}
function Epley(r, w) {
        return w * (1 + r / 30);
    }
    function Brzycki(r, w) {
        return w * 36 / (37 - r);
    }
    function Lander(r, w) {
        return (100 * w) / (101.3 - 2.67123 * r);
    }
    function Lombardi(r, w) {
        return w * Math.pow(r, 0.1);
    }
    function Mayhew(r, w) {
        return (100 * w) / (52.2 + 41.9 * Math.pow(Math.E, -0.055 * r));
    }
    function OConner(r, w) {
        return w * (1 + 0.025 * r);
    }
    function Wathan(r, w) {
        return (100 * w) / (48.8 + 53.8 * Math.pow(Math.E, -0.075 * r));
    }
    function EpleyReverse(r, max) {
        return max / (1 + r / 30);
    }
    function BrzyckiReverse(r, max) {
        return max / (36 / (37 - r));
    }
    function LanderReverse(r, max) {
        return (max * (101.3 - 2.67123 * r)) / 100;
    }
    function LombardiReverse(r, max) {
        return max / Math.pow(r, 0.1);
    }
    function MayhewReverse(r, max) {
        return (max * (52.2 + 41.9 * Math.pow(Math.E, -0.055 * r))) / 100;
    }
    function OConnerReverse(r, max) {
        return max / (1 + 0.025 * r);
    }
    function WathanReverse(r, max) {
        return (max * (48.8 + 53.8 * Math.pow(Math.E, -0.075 * r))) / 100;
    }
</script>

<style lang="stylus">
</style>
