export function normalizeData(obj: any): any {
  const result: any = {};
  for (const key in obj) {
    if (obj.hasOwnProperty(key)) {
      const normalizedKey = key.replace(/_([a-z])/g, (_, char) =>
        char.toUpperCase()
      );
      const capitalizedKey =
        normalizedKey.charAt(0).toUpperCase() + normalizedKey.slice(1);
      result[capitalizedKey] = obj[key];
    }
  }
  return result;
}
