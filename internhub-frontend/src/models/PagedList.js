export class PagedList {
  constructor({
    currentPage = 1,
    lastPage = 1,
    databaseRecordsCount = 0,
    pageSize = 0,
    listSize = 0,
    data = [],
  }) {
    this.currentPage = currentPage;
    this.lastPage = lastPage;
    this.databaseRecordsCount = databaseRecordsCount;
    this.pageSize = pageSize;
    this.listSize = listSize;
    this.data = data;
  }

  static fromJson(json, dataList) {
    return new PagedList({
      currentPage: json["CurrentPage"],
      lastPage: json["LastPage"],
      databaseRecordsCount: json["DatabaseRecordsCount"],
      pageSize: json["PageSize"],
      listSize: json["ListSize"],
      data: dataList,
    });
  }
}
