export function enumName(map, value) {
  return Object.prototype.hasOwnProperty.call(map, value) ? map[value] : "unknown(" + value + ")";
}

export function optionNames(map, value) {
  if (Object.prototype.hasOwnProperty.call(map, value)) return map[value];
  const names = [];
  Object.keys(map).forEach(k => {
    const bit = Number(k);
    if (bit !== 0 && (value & bit) === bit) names.push(map[k]);
  });
  return names.length > 0 ? names.join("|") : "unknown(" + value + ")";
}
