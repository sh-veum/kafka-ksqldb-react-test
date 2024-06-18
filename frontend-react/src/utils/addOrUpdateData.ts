/**
 * Add or update an item in the array based on a unique identifier.
 *
 * @param {Array<T>} oldData - The original array of data.
 * @param {T} newData - The new data to add or update.
 * @param {(item: T) => any} keyExtractor - A function to extract the unique key from an item.
 * @returns {Array<T>} - The updated array of data.
 */
export function addOrUpdateData<T>(
  oldData: T[],
  newData: T,
  keyExtractor: (item: T) => unknown
): T[] {
  const existingItem = oldData.find(
    (item) => keyExtractor(item) === keyExtractor(newData)
  );

  if (existingItem) {
    return oldData.map((item) =>
      keyExtractor(item) === keyExtractor(newData) ? newData : item
    );
  } else {
    return [...oldData, newData];
  }
}
