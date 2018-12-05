import constants from '../../store/constants'
import api from '../../api'
import GraphBar from '../../components/graph-bar'
import moment from 'moment'
import graph from '../../graph'
//import Help from './exercise-progress-help'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    components: {
        GraphBar,
        //'exercise-progress-help': Help
    },
    data () {
      return {
        start: undefined,
        end: undefined,
        data: undefined,
        options: undefined,

        history: [],
        expenditureId: 'expenditure',
        selectedNutrients: [
          constants.ENERGY_ID,
          'expenditure',
          undefined,
          undefined
        ],
        nutrients: []
      }
    },
    computed:{
    },
    methods: {
        showMonths(months){
            this.end = new Date();
            this.start = moment(this.end).subtract(months, 'months').toDate();
            this.loadData();
        },
        loadData(){
            api.getNutrientHistory(this.start, this.end).then(response => {
                var data = response.data;
                if(data.length == 0){
                    this.data = undefined;
                    return;
                }
                this.history = data;
                this.showGraph();
            }).catch(_ => {
                this.notifyError(this.$t('fetchFailed'));
            }).finally(() => {
                this.$store.commit(constants.LOADING_DONE);
            });
        },
        showGraph() {
          var start = moment(this.history[0].date).startOf('day');
          var end = moment(this.history[this.history.length-1].date).endOf('day');
          var labels = this.history.map(d => new Date(d.date));
          var datasets = [];
          var yAxis = [];
          this.selectedNutrients.forEach((n, index) => {
            if(!n){
              return false;
            }
            var data;
            if(n === this.expenditureId){
              data = this.history.map(h => h.energyExpenditure);
            }
            else {
              data = this.history.map(h => h.nutrients[n]);
            }
            var option = this.nutrients.find(o => o.value == n);
 
            datasets.push({
              ...graph.datasets[index],
              label: option.label,
              data: data,
              yAxisID: option.axisLabel,
              type: 'line'
            });

            var axis = yAxis.find(a => a.id == option.axisLabel);
            if(!axis){
              yAxis.push({
                id: option.axisLabel,
                position: yAxis.length % 2 == 0 ? 'left' : 'right',
                scaleLabel: {
                  display: true,
                  labelString: option.axisLabel
                },
                /*
                gridLines: {
                  display: yAxis.length % 2 == 0
                  //color: graph.axisColor2
                },
                */
                fill: false,
                ticks: {
                  //max: 5,
                  min: 0,
                  //max: max,
                  //stepSize: step 
                }
              });
            }

          });

    
          

          this.data = {
            labels,
            datasets
          };
          
          this.options = {
            maintainAspectRatio: false,
            legend: {
              display: true
            },
            scales: {
              xAxes:[{
                type: 'time',
                time: {
                  unit: 'day',
                  displayFormats: {
                      day: 'DD.MM.YYYY',
                      //hour: 'HH:mm'
                  },
                  min: start,
                  max: end
                }
              }],
              yAxes: yAxis
            },
            tooltips:{
              callbacks:{
                title: (tooltipItem, data) => {
                  var time = data.labels[tooltipItem[0].index];
                  return this.formatDateTime(time);
                }
              }
            }
          };
        

        },
        showHelp(){
          this.$refs.help.open();
        }
    },
    created() {
      this.end = moment().endOf('day').toDate();
      this.start = moment(this.end).subtract(6,'month').startOf('day').toDate();

      this.$store.dispatch(constants.FETCH_NUTRIENTS,{}).then(nutrients => {
        var nutrientOptions = [
          {
            label: '',
            value: undefined
          },
          {
            label: `${this.$t('energyExpenditure')} (${this.formatUnit('KCAL')})`,
            value: this.expenditureId,
            axisLabel: this.formatUnit('KCAL')
          }
        ];
        this.$store.state.nutrition.nutrientGroups.forEach(g => {
          nutrientOptions.push({
            label: g.name,
            value: 'group-' + g.id,
            disable: true
          });
          nutrients.filter(n => n.fineliGroup == g.id).sort((n1, n2) => n1.name < n2.name ? -1 : 1).forEach(n => {
            nutrientOptions.push({
              label: `${n.name} (${this.formatUnit(n.unit)})`,
              value: n.id,
              axisLabel: this.formatUnit(n.unit)
            });
          });
        });
        this.nutrients = nutrientOptions;
        this.loadData();
      });
        
    }
}