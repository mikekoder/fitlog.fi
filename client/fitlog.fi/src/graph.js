const pointRadius = 5;
const pointHitRadius = 10;
const dataset = {
    fill: false,
    pointRadius,
    pointHitRadius
};

const color1 = '#b6d957';
const color2 = '#fac364';
const color3 = '#d998cb';
const color4 = '#5cbae6';

const color5 = '#abdbf2';
export default {   
    datasets: [
        {
          ...dataset,
          borderColor: color1,
          pointBackgroundColor: color1,
          backgroundColor: color1
        },
        {
          ...dataset,
          borderColor: color2,
          pointBackgroundColor: color2,
          backgroundColor: color2
        },
        {
          ...dataset,
          borderColor: color3,
          pointBackgroundColor: color3,
          backgroundColor: color3
        },
        {
          ...dataset,
          borderColor: color4,
          pointBackgroundColor: color4,
          backgroundColor: color4
        }
    ],
    axisColor2: color5
}