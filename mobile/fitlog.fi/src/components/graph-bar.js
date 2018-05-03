import { Bar, mixins } from 'vue-chartjs'

export default {
  name: 'graph-bar',
  extends: Bar,
  mixins: [mixins.reactiveProp],
  props: ['chartData', 'options'],
  watch: {
    options () {
       this.renderChart(this.chartData, this.options);
    }
  },
  mounted () {
    this.renderChart(this.chartData, this.options);
  }
}