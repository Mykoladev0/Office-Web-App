import classNames from 'classnames';

export class ClassNameBuilder {
  private namespace: any = '';
  constructor(namespace: any) {
    this.namespace = namespace;
  }
  public create(blockName: any) {
    let block = blockName;

    if (typeof this.namespace === 'string') {
      block = `${this.namespace}-${blockName}`;
    }

    return {
      b: (...more) => {
        return classNames(block, more);
      },
      e: (className, ...more) => {
        return classNames(`${block}__${className}`, more);
      },
      m: (className, ...more) => {
        return classNames(`${block}--${className}`, more);
      },
    };
  }
}
// export const createClassName = namespace => {
//   return {
//     create: blockName => {
//       let block = blockName;

//       if (typeof namespace === 'string') {
//         block = `${namespace}-${blockName}`;
//       }

//       return {
//         b: (...more) => {
//           return classNames(block, more);
//         },
//         e: (className, ...more) => {
//           return classNames(`${block}__${className}`, more);
//         },
//         m: (className, ...more) => {
//           return classNames(`${block}--${className}`, more);
//         },
//       };
//     },
//   };
// };

export const createClassNames = new ClassNameBuilder('cr');

export default createClassNames;
